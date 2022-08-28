import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ActivatedRoute } from '@angular/router';
import { ShoppingService } from 'src/app/services/shopping.service';
import { CookieService } from 'ngx-cookie-service';
import Swal from 'sweetalert2';
@Component({
  selector: 'app-view-product',
  templateUrl: './view-product.component.html',
  styleUrls: ['./view-product.component.css']
})
export class ViewProductComponent implements OnInit {

  

  token:string="";
  id:any=null;
  role:any;

  productId:any;
  ItemInfo:any=[];

  MaxIncrValue:any;
  CurrentValue:any=1;

  duplicateArray:any=[];

  cartItemId:any;
  existingCartQuantity:any;

  Price:any;
  constructor(private router: Router,private activatedRoute:ActivatedRoute,private service:ShoppingService,private cookieService: CookieService) {
   
   }

  ngOnInit(): void {
    
    this.productId=localStorage.getItem("ItemId");
    console.log(this.productId);
    if(this.productId==null){
      this.router.navigate(["products"]);
    }
    this.token=this.cookieService.get('token');
  this.id=this.cookieService.get('customerId');
  this.role=this.cookieService.get('role');
    this.getEditInfo();
  }

  getEditInfo(){
    
    this.service.getProductById(this.productId).subscribe(data=>{
      this.ItemInfo=data;
      
       this.MaxIncrValue=data[0].Quantity;
       this.Price=data[0].Price;
       
      
    });
    console.log(this.ItemInfo);
  }

  increment(){
    console.log( this.CurrentValue);
    if(this.CurrentValue<this.MaxIncrValue){
      this.CurrentValue++;
      
    }
  }

  decrement(){
    if(this.CurrentValue>1){
      this.CurrentValue--;
    }
  }

postId:any;
  addToCart(){
    console.log(this.id);
    if(this.id=="" || this.role=="admin"){
      Swal.fire({
        position: 'center',
        icon: 'warning',
        title: 'Please Login To Add To Cart',
        showConfirmButton: false,
        timer: 1500
      })
      
      this.router.navigate(['login'], {
        state:{
          data: this.productId
        }
      });
    }else{
      
      const Duplybody={ ProductId:this.productId,CustomerId:this.id}
      //Check If Item Already Exist In The Cart
      this.service.checkDuplication(Duplybody,this.token).subscribe(data=>{
        this.duplicateArray=data;
        if(this.duplicateArray.length>0){
         this.cartItemId=data[0].ID;
         this.existingCartQuantity=data[0].Quantity;

         //UpdateExisting
         const updatebody={ ItemsId:this.cartItemId,Quantity:this.existingCartQuantity+this.CurrentValue }
    this.service.updateQuantity(updatebody,this.token).subscribe(data=>{
      this.postId=data;
      if(this.postId!=1){
        Swal.fire({
          position: 'center',
          icon: 'warning',
          title: 'Unable To Add To Cart',
          showConfirmButton: false,
          timer: 1500
        }).then(() => {
          window.location.reload();
          
        });
      }else{
        window.location.reload();
        Swal.fire({
          position: 'center',
          icon: 'success',
          title: 'Cart Updated',
          showConfirmButton: false,
          timer: 1500
        }).then(() => {
          window.location.reload();
          
        });
        
      }
    })

        }else{
          const body={ ProductId:this.productId,CustomerId:this.id,Quantity:this.CurrentValue,Price:this.Price }
    this.service.addToCart(body,this.token).subscribe(data=>{
      this.postId=data;
      if(this.postId!=1){
        Swal.fire({
          position: 'center',
          icon: 'warning',
          title: 'Unable To Add To Cart',
          showConfirmButton: false,
          timer: 1500
        }).then(() => {
          window.location.reload();
          this.router.navigate(["products"]);
        });
      }else{
        Swal.fire({
          position: 'center',
          icon: 'success',
          title: 'Cart Updated',
          showConfirmButton: false,
          timer: 1500
        }).then(() => {
          window.location.reload();
          this.router.navigate(["products"]);
        });
      }
    })
        }
      })


      
  }
}
  
}
