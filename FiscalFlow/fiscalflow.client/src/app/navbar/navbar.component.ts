import { Component } from '@angular/core';
import { UserService } from '../user/user.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent {
  user$ = this.userService.user$;

  constructor(public userService: UserService) {}

  logout() {
    this.userService.logout();
  }
}
