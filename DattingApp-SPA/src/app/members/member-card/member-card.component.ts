import { Component, OnInit, Input } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AuthService } from 'src/app/_service/auth.service';
import { AlertifyService } from 'src/app/_service/alertify.service';
import { UserService } from 'src/app/_service/user.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;
  constructor(
    private authService: AuthService,
    private usrService: UserService,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {}
  sendLike(id: number) {
    this.usrService.sendLike(
      this.authService.decodedToken.nameid, id).subscribe(data => {
        this.alertify.success('You\'ve liked: ' + this.user.knownAs);
      }, error => {
        this.alertify.error(error);
      });
  }
}
