import { Component, OnInit } from '@angular/core';

import { Router } from '@angular/router';
import { ShoppingService } from 'src/app/services/shopping.service';
import { ViewProductComponent } from '../view-product/view-product.component';
import { CookieService } from 'ngx-cookie-service';
@Component({
  selector: 'app-display-products',
  templateUrl: './display-products.component.html',
  styleUrls: ['./display-products.component.css']
})
export class DisplayProductsComponent implements OnInit {

  constructor(private service:ShoppingService,private router: Router,private cookieService: CookieService) { }

  AllProductList:any=[];
  pageOfProducts: Array<any>=[];
  category:any="All"

  categoryList:any=[];
 

  ngOnInit(): void {

    if(this.cookieService.check('token')){
      
       
      this.refreshAllproducts();
   this.getcategoryList();
      
      
      
    }else{
      this.refreshAllproducts();
   this.getcategoryList();
    }

    
  }


  refreshAllproducts(){
    this.category="All";
    this.service.getAllProducts().subscribe(data=>{
      this.AllProductList=data;
      
    });
    
  
  }

  filterbycategory(categoryName:any){
    this.category=categoryName;
    this.service.filterByCategory(categoryName).subscribe(data=>{
      this.AllProductList=data;
      
    });
  }

  getcategoryList(){
    this.service.getCategoryList().subscribe(data=>{
      this.categoryList=data;
      console.log(data);
      
    });
  }

  searchChange(event:any){
    console.log(event.target.value);
    if(event.target.value.length>0){
      this.filterbySearch(event.target.value);
    }
    
  }


  filterbySearch(search:any){
    this.category="All";
    this.service.filterBySearch(search).subscribe(data=>{
      this.AllProductList=data;
      
    });
  }

  onChangePage(pageOfItems: Array<any>) {
    // update current page of products
    this.pageOfProducts = pageOfItems;
}


viewItemDetails(productId:any){
  localStorage.setItem("ItemId", productId);
  this.router.navigate(['item'], {
    state:{
      data: productId
    }
  });

}

}
