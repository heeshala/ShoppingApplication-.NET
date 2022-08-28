import { Component,OnInit } from '@angular/core';
import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';
import { ShoppingService } from './services/shopping.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'Shopper.LK';

  token:any;
  id:any;
  CartItems:any=[];
  ItemsCount:any;
  CustomerName:any;
  role:any;
  AdminName:any;


  constructor(private cookieService: CookieService,private router: Router,private service:ShoppingService){}
  ngOnInit() {
    this.token=this.cookieService.get('token');
    this.id=this.cookieService.get('customerId');
    this.CustomerName=this.cookieService.get('customerName');
    this.AdminName=this.cookieService.get('adminName');
    this.role=this.cookieService.get('role');
    if(this.role=='admin'){
      //this.router.navigate(["dashboard"]);
    }
    if(this.role=='customer'){
      this.service.getCart(this.id,this.token).subscribe(data=>{
        this.CartItems=data;
        this.ItemsCount=this.CartItems.length;
      });
    }
    
   
    
    }

    onCart(){
      this.router.navigate(["cart"]);
    }

    logout(){
      this.cookieService.delete('token');
      this.cookieService.delete('customerId');
      this.cookieService.delete('customerName');

      this.router.navigate(["products"]);
      window.location.reload();
      
    }

    shop(){
      this.router.navigate(["products"]).then(() => {
        window.location.reload();
      });
    }

    orders(){
      this.router.navigate(["orders"]).then(() => {
        window.location.reload();
      });
    }

    dashboard(){
      this.router.navigate(["dashboard"]).then(() => {
        window.location.reload();
      });
    }
  
}
