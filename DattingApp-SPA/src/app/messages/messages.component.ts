import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/message';
import { Pagination, PaginatedResult } from '../_models/pagination';
import { AuthService } from '../_service/auth.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../_service/alertify.service';
import { UserService } from '../_service/user.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';
  constructor(
    private userService: UserService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private alertify: AlertifyService
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.messages = data.messages.result;
      this.pagination = data.messages.pagination;
    });
  }
  loadMessages() {
    this.userService
      .getMessages(
        this.authService.decodedToken.nameid,
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.messageContainer
      )
      .subscribe(
        (res: PaginatedResult<Message[]>) => {
          this.messages = res.result;
          this.pagination = res.pagination;
        },
        error => {
          this.alertify.error('Load Messages --> ' + error);
        }
      );
  }
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }
  deleteMessage(msgId: number) {
    this.alertify.confirm('Are you sure you want to delete the message?', () => {
      this.userService.deleteMessage(msgId, this.authService.decodedToken.nameid).subscribe(() => {
        const msgIx: number = this.messages.findIndex(m => m.id === msgId);
        this.messages.splice(msgIx, 1);
        this.alertify.success('Message deleted!');
      }, error => {
        this.alertify.error('Failed to delete message -->' + error);
      });
    });
  }
}
