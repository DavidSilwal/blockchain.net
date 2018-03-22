import { Component, Inject, AfterViewInit } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'wallet',
    templateUrl: './wallet.component.html'
})
export class WalletComponent implements AfterViewInit {

    public walletBalance: WalletBalance;

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string) {

    }

    ngAfterViewInit() {
        //this.reloadWalletBalance();
    }

    public startWalletBalance() {
        this.reloadWalletBalance();
    }

    private reloadWalletBalance() {
        this.http.get(this.baseUrl + 'api/wallet/WalletBalance').subscribe(result => {
            this.walletBalance = result.json() as WalletBalance;
            setTimeout(() => this.reloadWalletBalance(), 4000);
        }, error => console.error(error));
    }

    public username = 'wallet';
    public passwort = 'test123';
}

interface WalletBalance {
    balance: number;
}
