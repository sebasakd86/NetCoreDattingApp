import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_service/user.service';
import { AlertifyService } from '../../_service/alertify.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  constructor(private usrService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadUsers();
  }
  loadUsers() {
    this.usrService.getUsers().subscribe(
      (users: User[]) => {
        console.log(users);
        this.users = users;
      },
      error => {
        this.alertify.error(error);
      });
  }
}
