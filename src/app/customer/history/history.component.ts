import { Component, OnInit ,OnDestroy } from '@angular/core';

import { CookieService } from 'ngx-cookie-service';

import { CustomerService } from 'src/app/services/customer.service';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-history',
  templateUrl: './history.component.html',
  styleUrls: ['./history.component.css']
})
export class HistoryComponent implements OnInit  {

  constructor(private service:CustomerService,private cookieService: CookieService,private router: Router) { }

  HistoryList:any=[];
  BillInfo:any=[];
  token:string="";
  id:any;
  totalBill:any=0;
  dateofPurchase:any="";
  address:any="";
  city:any="";
  zip:any="";

  dtOptions: DataTables.Settings = {};
  

  ngOnInit(): void {
    
    if(this.cookieService.check('token')){
      
      this.HistoryList=[];
    console.log(this.HistoryList);
    this.token=this.cookieService.get('token');
    console.log(this.token);
  this.id=this.cookieService.get('customerId');
    this.getAllHistory();
    
    
      
    }else{
      console.log("nocookie")
      this.router.navigate(["login"]);
    }
    
  }

  
  

  getAllHistory(){
    this.service.getHistory(this.id,this.token).subscribe(data=>{
      this.HistoryList=data;
      
      
    });
  }

  viewbill(invoiceid:any){
    this.service.getInvoice(invoiceid,this.token).subscribe(data=>{
      this.BillInfo=data;
      console.log(this.BillInfo);
      
      this.dateofPurchase=this.BillInfo[0]["OrderDate"];
      this.address=this.BillInfo[0]["Address"];
      this.city=this.BillInfo[0]["City"];
      this.zip=this.BillInfo[0]["Zip"];
      this.getTotal();
    });
  }

  //get Total
  getTotal(){
    this.totalBill=0;
    console.log(this.BillInfo.length);
    for(var a=0;a<this.BillInfo.length;a++){
     this.totalBill+=(this.BillInfo[a]["Quantity"]*this.BillInfo[a]["Price"]);
     
    }
    
   }
   
  
}
