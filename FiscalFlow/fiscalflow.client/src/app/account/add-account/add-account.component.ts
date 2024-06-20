import { Component, Input, OnInit } from '@angular/core';
import { CreateAccountRequest } from '../../shared/models/account/createAccount';
import { CurrencyEnum } from '../../shared/models/account/enums/currencyEnum';
import { AccountType } from '../../shared/models/account/enums/accountType';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../account.service';
import { SharedService } from '../../shared/shared.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Account } from 'src/app/shared/models/account/account';
import { UpdateAccount } from 'src/app/shared/models/account/updateAccount';

@Component({
  selector: 'app-add-account',
  templateUrl: './add-account.component.html',
  styleUrls: ['./add-account.component.scss'],
})
export class AddAccountComponent implements OnInit {
  accountForm: FormGroup = new FormGroup({});
  isEditMode: boolean;
  oldBalance: number | null=  0;
  currencyValues = Object.values(CurrencyEnum).slice(0, 173);
  currencyIdMapping: { [key: string]: CurrencyEnum } = {
    AED: CurrencyEnum.AED,
    AFN: CurrencyEnum.AFN,
    ALL: CurrencyEnum.ALL,
    AMD: CurrencyEnum.AMD,
    ANG: CurrencyEnum.ANG,
    AOA: CurrencyEnum.AOA,
    ARS: CurrencyEnum.ARS,
    AUD: CurrencyEnum.AUD,
    AWG: CurrencyEnum.AWG,
    AZN: CurrencyEnum.AZN,
    BAM: CurrencyEnum.BAM,
    BBD: CurrencyEnum.BBD,
    BDT: CurrencyEnum.BDT,
    BGN: CurrencyEnum.BGN,
    BHD: CurrencyEnum.BHD,
    BIF: CurrencyEnum.BIF,
    BMD: CurrencyEnum.BMD,
    BND: CurrencyEnum.BND,
    BOB: CurrencyEnum.BOB,
    BOV: CurrencyEnum.BOV,
    BRL: CurrencyEnum.BRL,
    BSD: CurrencyEnum.BSD,
    BTN: CurrencyEnum.BTN,
    BWP: CurrencyEnum.BWP,
    BYN: CurrencyEnum.BYN,
    BZD: CurrencyEnum.BZD,
    CAD: CurrencyEnum.CAD,
    CDF: CurrencyEnum.CDF,
    CHE: CurrencyEnum.CHE,
    CHF: CurrencyEnum.CHF,
    CHW: CurrencyEnum.CHW,
    CLF: CurrencyEnum.CLF,
    CLP: CurrencyEnum.CLP,
    CNY: CurrencyEnum.CNY,
    COP: CurrencyEnum.COP,
    COU: CurrencyEnum.COU,
    CRC: CurrencyEnum.CRC,
    CUC: CurrencyEnum.CUC,
    CUP: CurrencyEnum.CUP,
    CVE: CurrencyEnum.CVE,
    CZK: CurrencyEnum.CZK,
    DJF: CurrencyEnum.DJF,
    DKK: CurrencyEnum.DKK,
    DOP: CurrencyEnum.DOP,
    DZD: CurrencyEnum.DZD,
    EGP: CurrencyEnum.EGP,
    ERN: CurrencyEnum.ERN,
    ETB: CurrencyEnum.ETB,
    EUR: CurrencyEnum.EUR,
    FJD: CurrencyEnum.FJD,
    FKP: CurrencyEnum.FKP,
    FOK: CurrencyEnum.FOK,
    GBP: CurrencyEnum.GBP,
    GEL: CurrencyEnum.GEL,
    GGP: CurrencyEnum.GGP,
    GHS: CurrencyEnum.GHS,
    GIP: CurrencyEnum.GIP,
    GMD: CurrencyEnum.GMD,
    GNF: CurrencyEnum.GNF,
    GTQ: CurrencyEnum.GTQ,
    GYD: CurrencyEnum.GYD,
    HKD: CurrencyEnum.HKD,
    HNL: CurrencyEnum.HNL,
    HRK: CurrencyEnum.HRK,
    HTG: CurrencyEnum.HTG,
    HUF: CurrencyEnum.HUF,
    IDR: CurrencyEnum.IDR,
    ILS: CurrencyEnum.ILS,
    IMP: CurrencyEnum.IMP,
    INR: CurrencyEnum.INR,
    IQD: CurrencyEnum.IQD,
    IRR: CurrencyEnum.IRR,
    ISK: CurrencyEnum.ISK,
    JEP: CurrencyEnum.JEP,
    JMD: CurrencyEnum.JMD,
    JOD: CurrencyEnum.JOD,
    JPY: CurrencyEnum.JPY,
    KES: CurrencyEnum.KES,
    KGS: CurrencyEnum.KGS,
    KHR: CurrencyEnum.KHR,
    KID: CurrencyEnum.KID,
    KMF: CurrencyEnum.KMF,
    KRW: CurrencyEnum.KRW,
    KWD: CurrencyEnum.KWD,
    KYD: CurrencyEnum.KYD,
    KZT: CurrencyEnum.KZT,
    LAK: CurrencyEnum.LAK,
    LBP: CurrencyEnum.LBP,
    LKR: CurrencyEnum.LKR,
    LRD: CurrencyEnum.LRD,
    LSL: CurrencyEnum.LSL,
    LYD: CurrencyEnum.LYD,
    MAD: CurrencyEnum.MAD,
    MDL: CurrencyEnum.MDL,
    MGA: CurrencyEnum.MGA,
    MKD: CurrencyEnum.MKD,
    MMK: CurrencyEnum.MMK,
    MNT: CurrencyEnum.MNT,
    MOP: CurrencyEnum.MOP,
    MRU: CurrencyEnum.MRU,
    MUR: CurrencyEnum.MUR,
    MVR: CurrencyEnum.MVR,
    MWK: CurrencyEnum.MWK,
    MXN: CurrencyEnum.MXN,
    MXV: CurrencyEnum.MXV,
    MYR: CurrencyEnum.MYR,
    MZN: CurrencyEnum.MZN,
    NAD: CurrencyEnum.NAD,
    NGN: CurrencyEnum.NGN,
    NIO: CurrencyEnum.NIO,
    NOK: CurrencyEnum.NOK,
    NPR: CurrencyEnum.NPR,
    NZD: CurrencyEnum.NZD,
    OMR: CurrencyEnum.OMR,
    PAB: CurrencyEnum.PAB,
    PEN: CurrencyEnum.PEN,
    PGK: CurrencyEnum.PGK,
    PHP: CurrencyEnum.PHP,
    PKR: CurrencyEnum.PKR,
    PLN: CurrencyEnum.PLN,
    PYG: CurrencyEnum.PYG,
    QAR: CurrencyEnum.QAR,
    RON: CurrencyEnum.RON,
    RSD: CurrencyEnum.RSD,
    RUB: CurrencyEnum.RUB,
    RWF: CurrencyEnum.RWF,
    SAR: CurrencyEnum.SAR,
    SBD: CurrencyEnum.SBD,
    SCR: CurrencyEnum.SCR,
    SDG: CurrencyEnum.SDG,
    SEK: CurrencyEnum.SEK,
    SGD: CurrencyEnum.SGD,
    SHP: CurrencyEnum.SHP,
    SLL: CurrencyEnum.SLL,
    SOS: CurrencyEnum.SOS,
    SPL: CurrencyEnum.SPL,
    SRD: CurrencyEnum.SRD,
    SSP: CurrencyEnum.SSP,
    STN: CurrencyEnum.STN,
    SVC: CurrencyEnum.SVC,
    SYP: CurrencyEnum.SYP,
    SZL: CurrencyEnum.SZL,
    THB: CurrencyEnum.THB,
    TJS: CurrencyEnum.TJS,
    TMT: CurrencyEnum.TMT,
    TND: CurrencyEnum.TND,
    TOP: CurrencyEnum.TOP,
    TRY: CurrencyEnum.TRY,
    TTD: CurrencyEnum.TTD,
    TWD: CurrencyEnum.TWD,
    TZS: CurrencyEnum.TZS,
    UAH: CurrencyEnum.UAH,
    UGX: CurrencyEnum.UGX,
    USD: CurrencyEnum.USD,
    USN: CurrencyEnum.USN,
    UYI: CurrencyEnum.UYI,
    UYU: CurrencyEnum.UYU,
    UZS: CurrencyEnum.UZS,
    VES: CurrencyEnum.VES,
    VND: CurrencyEnum.VND,
    VUV: CurrencyEnum.VUV,
    WST: CurrencyEnum.WST,
    XAF: CurrencyEnum.XAF,
    XCD: CurrencyEnum.XCD,
    XDR: CurrencyEnum.XDR,
    XOF: CurrencyEnum.XOF,
    XPF: CurrencyEnum.XPF,
    XSU: CurrencyEnum.XSU,
    XUA: CurrencyEnum.XUA,
    YER: CurrencyEnum.YER,
    ZAR: CurrencyEnum.ZAR,
    ZMW: CurrencyEnum.ZMW,
    ZWL: CurrencyEnum.ZWL,
  };

  constructor(
    private formBuilder: FormBuilder,
    private accountService: AccountService,
    private sharedService: SharedService,
    private router: Router,
    private route: ActivatedRoute
  ) {}
  ngOnInit() {
    this.accountForm = this.formBuilder.group({
      name: ['', Validators.required],
      balance: ['', Validators.required],
      currency: ['', Validators.required],
      accountType: ['Cash', Validators.required],
    });
    this.route.params.subscribe((params) => {
      if (params['id']) {
        this.isEditMode = true;
        this.accountService.getAccountId(params['id']).subscribe({
          next: (account: Account) => {
            console.log('Account:', account);
            this.oldBalance = account.balance;
            this.accountForm.patchValue({
              name: account.name,
              balance: account.balance,
              currency: account.currency,
              accountType:
                //@ts-ignore
                account.type === 'Bank' ? '1' : '0',
            });
          },
        });
      }
    });
  }
  onSubmit() {
    if (this.accountForm.valid) {
      if (this.isEditMode) {
        const formData: UpdateAccount = {
          name: this.accountForm.value.name,
          moneyCurrency: this.currencyIdMapping[this.accountForm.value.currency],
          moneyBalance: Number(this.accountForm.value.balance) === this.oldBalance ? null : Number(this.accountForm.value.balance),
          accountType: Number(this.accountForm.value.accountType),
        };
        console.log(formData);
        this.accountService
          .updateAccount(this.route.snapshot.params['id'], formData)
          .subscribe({
            next: (response: any) => {
              this.sharedService.showNotification(
                true,
                'Account update',
                'Account was updated with success!'
              );
              this.router.navigateByUrl('/');
            },
            error: (response: any) => {
              const errorMessage: string = response.error.detail;
              const displayMessage: string = errorMessage
                .substring(errorMessage.indexOf('*') + 1)
                .trim();
              this.sharedService.showNotification(
                false,
                'Account update',
                displayMessage
              );
            },
          });
      } else {
        let formData: CreateAccountRequest = this.accountForm.value;
        formData.accountType = Number(formData.accountType);
        this.accountService.addAccount(formData).subscribe({
          next: (response: any) => {
            this.sharedService.showNotification(
              true,
              'Account creation',
              'Account was created with success!'
            );
            this.router.navigateByUrl('/');
          },
          error: (response: any) => {
            const errorMessage: string = response.error.detail;
            const displayMessage: string = errorMessage
              .substring(errorMessage.indexOf('*') + 1)
              .trim();
            this.sharedService.showNotification(
              false,
              'Account creation',
              displayMessage
            );
          },
        });
      }
    }
  }
}
