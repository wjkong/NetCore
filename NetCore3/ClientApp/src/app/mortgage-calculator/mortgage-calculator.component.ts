import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-mortgage-calculator',
  templateUrl: './mortgage-calculator.component.html'
})



export class MortgageCalculatorComponent implements OnInit {

  yearArray: number[] = [];
  principal: number = 0;
  rate: number = 0;
  frequency: number = 12;
  amortization: number = 1;
  paymentAmtPerPeriod: number;
  payments: InstallPayment[];
  showDetail: boolean = false;
  http: HttpClient;
  baseUrl: string;

  
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.http = http;
    this.baseUrl = baseUrl;

    this.payments = [];

    for (var i = 1; i <= 30; i++) {
      this.yearArray.push(i);
    }
  }

  ngOnInit() {
  }

  calculate() {
    var paras = new Paras()
    paras.amortization = parseInt(this.amortization.toString());
    paras.frequency = parseInt(this.frequency.toString());
    paras.principal = parseFloat(this.principal.toString());
    paras.rate = parseFloat(this.rate.toString());

    const headers = { 'content-type': 'application/json' }

    this.http.post<InstallPayment[]>(this.baseUrl + 'api/mortgagecalculator', JSON.stringify(paras), { 'headers': headers }).subscribe(result => {
    
      this.showDetail = true;

      this.payments = result;
    }, error => console.error(error));

  }
}

class InstallPayment {
  description: string;
  monthlyPayment: number;
  installPayment: number;
  totalCostOfBorrow: number;
  totalPayment: number;
  freq: string;
  frequency: number;
  amountSave: number;
}

class Paras {
  principal: number = 0;
  rate: number = 0;
  frequency: number = 12;
  amortization: number = 1;
}
