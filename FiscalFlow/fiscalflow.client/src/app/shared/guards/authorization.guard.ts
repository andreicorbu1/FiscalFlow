import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { AccountService } from 'src/app/account/account.service';
import { SharedService } from '../shared.service';
import { Observable, map } from 'rxjs';
import { User } from '../models/account/user';
@Injectable({
  providedIn: 'root'
})
export class AuthorizationGuard {
  constructor(private accountService: AccountService,
    private sharedService: SharedService,
    private router: Router) { }
  
    canActivate(route: ActivatedRouteSnapshot,
      state: RouterStateSnapshot) : Observable<boolean> {
        return this.accountService.user$.pipe(
          map((user: User | null) => {
            if(user) {
              return true;
            } else {
              this.sharedService.showNotification(false, "Restricted Access", "You must be logged in to access this page.");
              this.router.navigate(['account/login'], {queryParams: {returnUrl: state.url}});
              return false;
            }
          }
        ));
      }
  };
