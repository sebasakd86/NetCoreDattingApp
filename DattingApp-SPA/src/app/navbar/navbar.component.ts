import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_service/auth.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  model: any = {};

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }
  login(){
    this.authService.login(this.model).subscribe(next => {
      console.log('Login OK!');
    },
    error => {
      console.log('Login FAILED!!');
    });
  }
  loggedIn(){
    const token = localStorage.getItem('token');
    return !!token; // if(token) return true;
  }
  logOut(){
    localStorage.removeItem('token');
    console.log('Logged out!!');
  }
}
