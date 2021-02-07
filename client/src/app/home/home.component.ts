import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode = false;


  constructor(private http: HttpClient, private accountService: AccountService) { }

  ngOnInit(): void {

  }
  
  registerModeToggle() : void {
    this.registerMode = !this.registerMode;
  }


  cancelRegisterMode = (status: boolean) : void => {this.registerMode = status};
   


}
