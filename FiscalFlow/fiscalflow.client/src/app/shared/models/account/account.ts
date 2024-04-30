import {CurrencyEnum} from "./enums/currencyEnum";
import {AccountType} from "./enums/accountType";

export interface Account {
  id: string;
  name: string;
  balance: number;
  currency: CurrencyEnum;
  type: AccountType;
  createdOnUtc: Date;
  ownerId: string;
}
