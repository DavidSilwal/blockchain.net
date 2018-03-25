import { Component, Inject, OnInit, OnDestroy } from '@angular/core';
import { Http, Headers } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import { DecimalPipe } from '@angular/common';
import 'rxjs/add/observable/interval';
import 'rxjs/add/operator/takeWhile';
import { Subscription } from 'rxjs/Subscription';

@Component({
    selector: 'wallet',
    templateUrl: './wallet.component.html'
})
export class WalletComponent implements OnInit {

    public walletBalance: WalletBalance;
    public transaction: TransactionRequest;
    public transactions: Transaction[] = [];
    public blockHeight: number = 0;
    public actualAddress: Address;
    private subscriptions: Subscription[] = [];
    public transactionError: string;

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {
        this.transaction = { address: '', amount: 0, message: '' };
        this.actualAddress = { key: '' };
    }

    ngOnDestroy() {
        for (let subscription of this.subscriptions) {
            subscription.unsubscribe();
        }
    }

    ngOnInit() {
        this.subscriptions.push(Observable.interval(8000)
            .takeWhile(() => true)
            .subscribe(i => {
                this.reloadWalletBalance();
            }));
        this.reloadWalletBalance();
        this.subscriptions.push(Observable.interval(4000)
            .takeWhile(() => true)
            .subscribe(i => {
                this.loadTransactions();
            }));
        this.loadTransactions();
    }

    public reloadWalletBalance() {
        this.http.get(this.baseUrl + 'api/wallet/WalletBalance').subscribe(result => {
            this.walletBalance = result.json() as WalletBalance;
        }, error => console.error(error));
    }

    public addTransaction() {
        this.http.post(this.baseUrl + 'api/wallet/AddTransaction', this.transaction).subscribe(result => {
            this.transactionError = '';
            this.transaction = { address: '', amount: 0, message: '' };
        }, error => { this.transactionError = JSON.parse(error._body).error; });
    }

    public loadTransactions() {
        this.http.get(this.baseUrl + 'api/wallet/ActualTransactions?blockHeight=' + this.blockHeight).subscribe(result => {
            var transactions = result.json() as Transaction[];
            for (let trans of transactions) {
                this.transactions.unshift(trans);
                this.blockHeight = trans.blockHeight;
            }
        }, error => console.error(error));
    }

    public generateAddress() {
        this.http.get(this.baseUrl + 'api/wallet/GenerateAddress').subscribe(result => {
            this.actualAddress = result.json() as Address;
        }, error => console.error(error));
    }
}

interface Address {
    key: string;
}

interface WalletBalance {
    balance: number;
}

interface TransactionRequest {
    address: string;
    amount: number;
    message: string;
}

interface Transaction {
    inputs: IO[];
    message: string;
    blockHeight: number;
    outputs: string[];
    amount: number;
    isIncome: boolean;
}

interface IO {
    key: string;
    amount: number;
}

