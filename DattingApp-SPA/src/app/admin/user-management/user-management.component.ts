import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_service/admin.service';
import { AlertifyService } from 'src/app/_service/alertify.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { ModalContainerComponent } from 'ngx-bootstrap/modal/public_api';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: User[];
  bsModalRef: BsModalRef;

  constructor(
    private admService: AdminService,
    private alertify: AlertifyService,
    private modalService: BsModalService
  ) {}

  ngOnInit() {
    this.getUsersWithRoles();
  }
  getUsersWithRoles() {
    this.admService.getUsersWithRoles().subscribe(
      (users: User[]) => {
        this.users = users;
      },
      error => {
        this.alertify.error(error);
      }
    );
  }

  editRolesModal(user: User) {
    const initialState = {
      user,
      roles: this.getRolesArray(user)
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, {
      initialState
    });
    this.bsModalRef.content.updateSelectedRoles.subscribe((values) => {
      const rolesToUpdate = {
        roleNames: [...values.filter( el => el.checked).map( el => el.name)]
      };
      if (rolesToUpdate) {
        this.admService.updateUserRoles(user, rolesToUpdate).subscribe(() => {
          user.roles = [...rolesToUpdate.roleNames];
        }, error => {
          this.alertify.error(error);
        });
      }
    });
  }
  private getRolesArray(user) {
    const roles = [];
    const usrRoles = user.roles;
    const avlRoles: any[] = [
      {
        name: 'Admin',
        value: 'Admin',
        checked: false
      },
      {
        name: 'Moderator',
        value: 'Moderator',
        checked: false
      },
      {
        name: 'Member',
        value: 'Member',
        checked: false
      },
      {
        name: 'VIP',
        value: 'VIP',
        checked: false
      }
    ];
    for (const aRole of avlRoles) {
      let isMatch = false;
      for (const uRole of usrRoles) {
        if (aRole.name === uRole) {
          isMatch = true;
          avlRoles.find(r => r.name === aRole.name).checked = true;
          break;
        }
      }
      roles.push(aRole);
    }
    return roles;
  }
}
