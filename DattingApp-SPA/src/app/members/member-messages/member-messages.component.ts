import { Component, OnInit, Input } from '@angular/core';
import { Message } from 'src/app/_models/message';
import { UserService } from 'src/app/_service/user.service';
import { AuthService } from 'src/app/_service/auth.service';
import { AlertifyService } from 'src/app/_service/alertify.service';
import { tap } from 'rxjs/operators';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMsg: any = {};

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private alertify: AlertifyService
  ) {}
  ngOnInit() {
    this.loadMessages();
  }
  loadMessages() {
    const currentUserId: number = +this.authService.decodedToken.nameid;
    this.userService
      .getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
      .pipe(
        // to do something b4 subscribing
        tap(messages => {
          // for (let i = 0; i < messages.length; i++) {
          for (const msg  of messages) {
            if (!msg.isRead && msg.recipientId === currentUserId) {
              this.userService.markAsRead(currentUserId, msg.id);
            }
          }
        })
      )
      .subscribe(
        msgs => {
          this.messages = msgs;
        },
        error => {
          this.alertify.error(error);
        }
      );
  }

  sendMessage() {
    this.newMsg.recipientId = this.recipientId;
    this.userService
      .sendMessage(this.authService.decodedToken.nameid, this.newMsg)
      .subscribe(
        (msg: Message) => {
          this.messages.unshift(msg);
          this.newMsg.content = '';
        },
        error => {
          this.alertify.error(error);
        }
      );
  }
}
