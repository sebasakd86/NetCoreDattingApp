import { Component, OnInit } from '@angular/core';
import { AuthService } from './_service/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  jwtHelper = new JwtHelperService();
  constructor(private auth: AuthService) {}
  ngOnInit(): void {
    const t = localStorage.getItem('token');
    const u: User = JSON.parse(localStorage.getItem('user'));
    if (t) {
      this.auth.decodedToken = this.jwtHelper.decodeToken(t);
    }
    if (u) {
      this.auth.currentUser = u;
      this.auth.changeMemberPhoto(u.photoUrl);
    }
  }
}
