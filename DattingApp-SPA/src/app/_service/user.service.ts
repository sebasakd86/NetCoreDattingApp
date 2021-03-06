import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) {}

  getUsers(
    page?,
    itemsPerPage?,
    usrParams?,
    likesParam?
  ): Observable<PaginatedResult<User[]>> {
    const pResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }
    if (usrParams != null) {
      params = params.append('minAge', usrParams.minAge);
      params = params.append('maxAge', usrParams.maxAge);
      params = params.append('gender', usrParams.gender);
      params = params.append('orderBy', usrParams.orderBy);
    }

    if (likesParam === 'Likers') {
      params = params.append('likers', 'true');
      params = params.append('likees', 'false');
    }
    if (likesParam === 'Likees') {
      params = params.append('likees', 'true');
      params = params.append('likers', 'false');
    }

    return this.http
      .get<User[]>(this.baseUrl + 'users', { observe: 'response', params })
      .pipe(
        map(response => {
          pResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            pResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return pResult;
        })
      );
  }
  getUser(id): Observable<User> {
    const u = this.http.get<User>(this.baseUrl + 'users/' + id);
    // console.log(u);
    return u;
  }

  updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto(userId: number, photoId: number) {
    // Post requirees something in the body, so the empty obj goes along.
    return this.http.post(
      this.baseUrl + 'users/' + userId + '/photos/' + photoId + '/setMain',
      {}
    );
  }
  deletePhoto(userId: number, photoId: number) {
    return this.http.delete(
      this.baseUrl + 'users/' + userId + '/photos/' + photoId
    );
  }

  sendLike(id: number, recipientId: number) {
    return this.http.post(
      this.baseUrl + 'users/' + id + '/like/' + recipientId,
      {}
    );
  }
  getMessages(id: number, page?, itemsPerPage?, messageContainer?) {
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

    let params = new HttpParams();

    params = params.append('MessageContainer', messageContainer);

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this.http
      .get<Message[]>(this.baseUrl + 'users/' + id + '/messages', {
        observe: 'response',
        params
      })
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') !== null) {
            paginatedResult.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
          }
          return paginatedResult;
        })
      );
  }

  getMessageThread(senderId: number, receiverId: number) {
    return this.http.get<Message[]>(
      this.baseUrl + 'users/' + senderId + '/messages/thread/' + receiverId
    );
  }
  sendMessage(senderId: number, message: Message) {
    return this.http.post(
      this.baseUrl + 'users/' + senderId + '/messages',
      message
    );
  }
  deleteMessage(msgId: number, userId: number) {
    return this.http.post(
      this.baseUrl + 'users/' + userId + '/messages/' + msgId,
      {}
    );
  }
  markAsRead(userId: number, msgId: number) {
    this.http
      .post(
        this.baseUrl + 'users/' + userId + '/messages/' + msgId + '/read',
        {}
      )
      .subscribe();
  }
}
