import { Component, OnInit } from '@angular/core';
import { User } from '../../_models/user';
import { UserService } from '../../_service/user.service';
import { AlertifyService } from '../../_service/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/app/_models/pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  user: User = JSON.parse(localStorage.getItem('user'));
  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' }
  ];
  usrParams: any = {};
  pagination: Pagination;

  constructor(
    private usrService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    // this.loadUsers();
    this.route.data.subscribe(data => {
      this.users = data.users.result; // data.users;
      this.pagination = data.users.pagination;
    });
    this.clearFilters();
  }

  private clearFilters() {
    this.usrParams.gender = this.user.gender === 'female' ? 'male' : 'female';
    this.usrParams.minAge = 18;
    this.usrParams.maxAge = 99;
    this.usrParams.orderBy = 'lastActive';
  }

  resetFilters() {
    this.clearFilters();
    this.loadUsers();
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }
  loadUsers() {
    this.usrService
      .getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, this.usrParams)
      .subscribe(
        (res: PaginatedResult<User[]>) => {
          // console.log(users);
          this.users = res.result; // users;
          this.pagination = res.pagination;
        },
        error => {
          this.alertify.error(error);
        }
      );
  }
}
