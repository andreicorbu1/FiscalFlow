import {AccountType} from "./enums/accountType";
import {CurrencyEnum} from "./enums/currencyEnum";

export interface CreateAccountRequest {
  name: string,
  balance: number,
  currency: CurrencyEnum,
  accountType: AccountType
}
