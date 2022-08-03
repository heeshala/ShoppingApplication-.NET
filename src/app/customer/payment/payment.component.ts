import { Component, OnInit } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';
//import validator and FormBuilder
import { FormGroup, Validators, FormBuilder } from '@angular/forms';

import { ShoppingService } from 'src/app/services/shopping.service';
import { ThisReceiver } from '@angular/compiler';
import Swal from 'sweetalert2';

@Component({
  selector: 'app-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.css']
})
export class PaymentComponent implements OnInit {
  //example:string;
  constructor(private cookieService: CookieService,private router: Router,private service:ShoppingService,private fb: FormBuilder){

    
  }

  //Validations
  //Create FormGroup
  numberOnly = "^0|[1-9]\d*";
requiredForm: FormGroup= this.fb.group({
  
  address: ['', Validators.required],
  city: ['', Validators.required],
  zip: ['', Validators.required],
  cardname: ['', Validators.required],
  cardnumber: ['', Validators.compose([Validators.required,Validators.pattern(this.numberOnly)])],
  month:['',Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(2),Validators.pattern(this.numberOnly)])],
  year:['',Validators.compose([Validators.required, Validators.minLength(4), Validators.maxLength(4),Validators.pattern(this.numberOnly)])],
  cvv:['',Validators.compose([Validators.required, Validators.minLength(2), Validators.maxLength(4),Validators.pattern(this.numberOnly)])],
});




  address:any;
  city:any;
  zip:any;
  cardname:any;
  cardnumber:any;
  month:any;
  year:any;
  cvv:any;

  postId:any;

  token:string=this.cookieService.get('token');
  id:any=this.cookieService.get('customerId');

  CartItems:any=[];
  totalPrice:any=0;
  ngOnInit(): void {
    if(this.cookieService.check('token')){
      
      
      this.getCartList();
      
    }else{
      console.log("nocookie")
      this.router.navigate(["login"]);
    }
  }

  getCartList(){
    console.log(this.id);
    this.service.getCart(this.id,this.token).subscribe(data=>{
      this.CartItems=data;
      this.getTotal();
    });
    console.log(this.CartItems);
  }

  //get Total
  getTotal(){
    for(var a=0;a<this.CartItems.length;a++){
     this.totalPrice+=(this.CartItems[a]["Quantity"]*this.CartItems[a]["UnitPrice"]);
    }
    if(this.totalPrice<1){
      this.router.navigate(["orders"]);
    }
   }

  
  pay(){
    const body={ Address:this.address,CustomerId:this.id,CardNumber:this.cardnumber,ExpMonth:this.month,ExpYear:this.year,Cvn:this.cvv,CardHolderName:this.cardname,TotalPrice:this.totalPrice,City:this.city,Zip:this.zip }
    this.service.purchase(body,this.token).subscribe(data=>{
      this.postId=data;
      console.error(this.postId);
          if(this.postId==0){
            Swal.fire({
              position: 'center',
              icon: 'error',
              title: 'Payment Unsuccesfull',
              showConfirmButton: false,
              timer: 1500
            }).then(() => {
              window.location.reload();
              this.router.navigate(["orders"]);
            });
            
          }else{
            Swal.fire({
              position: 'center',
              icon: 'success',
              title: 'Order Placed Succesfull',
              showConfirmButton: false,
              timer: 1500
            }).then(() => {
              window.location.reload();
              this.router.navigate(["orders"]);
            });
            
          }
     });
  }

}
