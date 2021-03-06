import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Photo } from 'src/app/_models/photo';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_service/auth.service';
import { UserService } from 'src/app/_service/user.service';
import { AlertifyService } from 'src/app/_service/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getMemberPhotoChange = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  response: string;
  baseUrl = environment.apiUrl;
  currentMainPhoto: Photo;

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  constructor(
    private authService: AuthService,
    private usrService: UserService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.initializeUploader();
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url:
        this.baseUrl +
        'users/' +
        this.authService.decodedToken.nameid +
        '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: true,
      maxFileSize: 10 * 1024 * 1024
    });
    // to avoid CORS issues with ng2 File Upload
    this.uploader.onAfterAddingFile = file => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };
        this.photos.push(photo);
        if(photo.isMain) {
          this.updateMainPhotoUrl(photo.url);
        }
      }
    };
  }
  private updateMainPhotoUrl(url: string) {
    this.authService.changeMemberPhoto(url);
    this.authService.currentUser.photoUrl = url;
    localStorage.setItem(
      'user',
      JSON.stringify(this.authService.currentUser)
    );
  }
  setMainPhoto(photo: Photo) {
    this.usrService
      .setMainPhoto(this.authService.decodedToken.nameid, photo.id)
      .subscribe(
        () => {
          // console.log('success!');
          this.currentMainPhoto = this.photos.filter(p => p.isMain === true)[0];
          this.currentMainPhoto.isMain = false;
          photo.isMain = true;
          // this.getMemberPhotoChange.emit(photo.url);
          this.updateMainPhotoUrl(photo.url);
        },
        error => {
          this.alertify.error(error);
        }
      );
  }
  deletePhoto(id: number) {
    this.alertify.confirm('Are you sure you want to delete the photo?', () => {
      this.usrService
        .deletePhoto(this.authService.decodedToken.nameid, id)
        .subscribe(
          () => {
            this.photos.splice(
              this.photos.findIndex(p => p.id === id),
              1
            );
            this.alertify.success('Photo deleted!');
          },
          error => {
            this.alertify.error('Failed to delete photo');
          }
        );
    });
  }
}
