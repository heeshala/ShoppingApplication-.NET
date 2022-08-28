import { Component, OnInit } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';

import { ShoppingService } from 'src/app/services/shopping.service';
import Swal from 'sweetalert2';
@Component({
  selector: 'app-edit-cart',
  templateUrl: './edit-cart.component.html',
  styleUrls: ['./edit-cart.component.css']
})
export class EditCartComponent implements OnInit {

  constructor(private cookieService: CookieService,private router: Router,private service:ShoppingService) { }
cookieValue:any;

CartItems:any=[];

ItemEditInfo:any=[];

MaxIncrValue:any=0;
CurrentQuantity:any=0;

totalPrice:any=0;

outOfStock:any=0;

  ngOnInit(): void {
    
    if(this.cookieService.check('token')){
      if(this.cookieService.get('role')=='customer'){
        console.log(this.cookieValue);
        this.getCartList();
      }else{
        console.log("nocookie")
      this.router.navigate(["login"]);
      }
      
      
    }else{
      console.log("nocookie")
      this.router.navigate(["login"]);
    }
  }
  token:string=this.cookieService.get('token');
  id:any=this.cookieService.get('customerId');

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
    if(this.CartItems[a]["MaximumAvailable"]==0){
      this.outOfStock++;
    }
    this.totalPrice+=(this.CartItems[a]["Quantity"]*this.CartItems[a]["UnitPrice"]);
   }
  }

  getEditInfo(itemid:any){
    
    this.service.getEditInfo(itemid,this.token).subscribe(data=>{
      this.ItemEditInfo=data;
      
       this.MaxIncrValue=data[0].MaximumAvailable;
       this.CurrentQuantity=data[0].Quantity;
      
    });
    console.log(this.ItemEditInfo);
  }

  deleteItem(id:any){
    console.log(id);
    this.service.deleteItem(id,this.token).subscribe(data=>{
      
      window.location.reload();
    });
  }

  //increment 
  increment(){
    if(this.CurrentQuantity<this.MaxIncrValue){
      this.CurrentQuantity++;
    }
  }

  //decrement 
  decrement(){
    if(this.CurrentQuantity>1){
      this.CurrentQuantity--;
    }
  }


  //Update Cart
  updateCart(itemid:any){
    const body={ ItemsId:itemid,Quantity:this.CurrentQuantity }
    this.service.updateQuantity(body,this.token).subscribe(data=>{
      
      window.location.reload();
    });
  }

  //ckeckout
  checkout(){
    if(this.outOfStock>0){
      Swal.fire({
        position: 'center',
        icon: 'warning',
        title: 'Remove The Item That is Out Of Stock to Proceed',
        showConfirmButton: false,
        timer: 3000
      });
    }else{
    this.router.navigate(["payment"],{ state: { example: 'bar',time: 'bar2' } });
    }
  }
}
