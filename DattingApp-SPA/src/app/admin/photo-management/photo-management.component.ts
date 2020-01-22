import { Component, OnInit } from '@angular/core';
import { AdminService } from 'src/app/_service/admin.service';
import { AlertifyService } from 'src/app/_service/alertify.service';
import { Photo } from 'src/app/_models/photo';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {
  photosToApprove: Photo[];
  constructor(
    private admService: AdminService,
    private alertify: AlertifyService
  ) { }

  ngOnInit() {
    this.loadPhotos();
  }

  loadPhotos() {
    this.admService.getUnapprovedPhotos().subscribe((d: Photo[]) => {
      this.photosToApprove = d;
    }, error => {
      // console.log(error);
      this.alertify.error(error);
    });
  }
  moderatePhoto(p: Photo, status: boolean) {
    // console.log(status);
    this.admService.moderatePhoto(p.id, status).subscribe(() => {
      const ix = this.photosToApprove.findIndex(pAux => pAux.id === p.id);
      this.alertify.success('Photo was moderated!');
      this.photosToApprove.splice(ix, 1);
    }, error => {
      this.alertify.error(error);
    });
  }
}
