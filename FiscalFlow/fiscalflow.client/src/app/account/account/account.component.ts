import {Component, Input} from '@angular/core';
import {Account} from "../../shared/models/account/account";

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.scss']
})
export class AccountComponent {
@Input() account: Account | undefined;

}
