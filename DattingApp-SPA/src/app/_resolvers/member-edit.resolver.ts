import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_service/user.service';
import { AlertifyService } from '../_service/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_service/auth.service';

@Injectable()

export class MemberEditResolver implements Resolve<User> {
    constructor(private usrService: UserService,
                private route: Router,
                private alertify: AlertifyService,
                private auth: AuthService) {}
    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.usrService.getUser(this.auth.decodedToken.nameid).pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving your data');
                this.route.navigate(['/members']);
                return of(null); // observable of null.
            })
        );
    }
}