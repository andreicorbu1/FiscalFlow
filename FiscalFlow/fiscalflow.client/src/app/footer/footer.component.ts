import { Component } from '@angular/core';
import { UserService } from '../user/user.service';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss'],
})
export class FooterComponent {
  currentYear: number = new Date().getFullYear();
  user$ = this.userService.user$;
  constructor(private userService: UserService) {}
}
