import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_service/user.service';
import { AlertifyService } from '../_service/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()

export class MemberListResolver implements Resolve<User[]> {
    constructor(private usrService: UserService,
                private route: Router,
                private alertify: AlertifyService) {}
    resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
        return this.usrService.getUsers().pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving data --> ' + error);
                this.route.navigate(['/home']);
                return of(null); // observable of null.
            })
        );
    }
}