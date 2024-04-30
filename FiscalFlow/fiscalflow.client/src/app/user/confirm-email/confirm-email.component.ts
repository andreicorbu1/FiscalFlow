import { Component, OnInit } from '@angular/core';
import { UserService } from '../user.service';
import { SharedService } from 'src/app/shared/shared.service';
import { ActivatedRoute, Router } from '@angular/router';
import { take } from 'rxjs';
import { User } from 'src/app/shared/models/user/user';
import { ConfirmEmail } from 'src/app/shared/models/user/confirmEmail';

@Component({
  selector: 'app-confirm-email',
  templateUrl: './confirm-email.component.html',
  styleUrls: ['./confirm-email.component.scss']
})
export class ConfirmEmailComponent implements OnInit {
success: boolean = true;

constructor(private userService: UserService,
  private sharedService: SharedService,
  private activatedRoute: ActivatedRoute,
  private router: Router) { }

  ngOnInit(): void {
    this.userService.user$.pipe(take(1)).subscribe({
      next: (user: User | null) => {
        if(user){
          this.router.navigateByUrl('/');
        } else {
          this.activatedRoute.queryParamMap.subscribe({
            next: (params: any) => {
              const confirmEmail: ConfirmEmail = {
                token: params.get('token'),
                email: params.get('email')
              };
              this.userService.confirmEmail(confirmEmail).subscribe({
                next:(response:any) => {
                  this.sharedService.showNotification(true, response.title, response.message);
                },
                error: error => {
                  this.success = false;
                  this.sharedService.showNotification(false, "Failed", error.error);
                }
              });
            }
          })
        }
      }
    })
  }

  resendEmailConfirmationLink() {
    this.router.navigateByUrl('/user/send-email/resend-email-confirmation-link');
  }

}
