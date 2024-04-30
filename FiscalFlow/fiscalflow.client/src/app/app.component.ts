import { Component, OnInit } from '@angular/core';
import { UserService } from './user/user.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit{

constructor(private userService: UserService){}

  ngOnInit(): void {
    this.refreshUser();
  }

  private refreshUser() {
    const jwt = this.userService.getJwt();
    if(jwt) {
      this.userService.refreshUser(jwt).subscribe({
        next: _ => {},
        error: _ => {
          this.userService.logout();
        }
      });
    } else {
      this.userService.refreshUser(null).subscribe();
    }
  }

  title = 'FiscalFlow';
}
