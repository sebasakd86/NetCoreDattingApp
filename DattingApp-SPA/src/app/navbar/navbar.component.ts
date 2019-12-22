import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_service/auth.service';
import { AlertifyService } from '../_service/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  model: any = {};

  constructor(public authService: AuthService, private alertify: AlertifyService, private router: Router) { }

  ngOnInit() {
  }
  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertify.success('Login OK!');
    },
    error => {
      this.alertify.error('Login Failed!');
    }, () => {
      this.router.navigate(['/members']);
    });
  }
  loggedIn() {
    // const token = localStorage.getItem('token');
    // return !!token; // if(token) return true;
    return this.authService.loggedIn();
  }
  logOut() {
    localStorage.removeItem('token');
    this.alertify.message('User logged out');
    this.router.navigate(['/home']);
  }
}
