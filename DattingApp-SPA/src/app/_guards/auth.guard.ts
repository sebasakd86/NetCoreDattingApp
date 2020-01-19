import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { AuthService } from '../_service/auth.service';
import { AlertifyService } from '../_service/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router, private alertify: AlertifyService){}
  canActivate(next: ActivatedRouteSnapshot): boolean {
    const roles = next.firstChild.data.roles as Array<string>;
    if (roles) { // if the guard holds a role in its data
      const match = this.authService.roleMatch(roles);
      if (match) {
        return true;
      }
      this.router.navigate(['members']);
      this.alertify.error('User unauthorized');
    }

    if (this.authService.loggedIn()) {
      return true;
    }
    this.alertify.error('User unauthorized');
  }
}
