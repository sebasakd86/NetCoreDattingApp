import { Component, OnInit } from '@angular/core';
import { AuthService } from './_service/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';

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
    if (t) {
      this.auth.decodedToken = this.jwtHelper.decodeToken(t);
    }
  }
}
