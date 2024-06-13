export interface Subscription {
  account: string;
  payee: string;
  value: number;
  currency: number;
  firstPayment: Date;
  lastPayment: Date;
  remainingPayments: number;
  id: string;
}
