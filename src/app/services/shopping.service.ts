import { Injectable } from '@angular/core';
import {HttpClient, HttpClientModule, HttpHeaders} from '@angular/common/http';
import { Router } from '@angular/router';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ShoppingService {

  readonly APIUrl="https://localhost:44323/api"
  errorMessage: any;

  static ItemId:any;

  constructor(private http:HttpClient,private router: Router) { }

   //Get All the Items List
   getAllProducts():Observable<any[]>{
    console.log("Calles");
    return this.http.get<any>(this.APIUrl+'/Shopping');
  }

  //Get Category
  getCategoryList():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/Shopping/GetCategoryList');
  }

  //Filter By category
  filterByCategory(category:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/Shopping/GetByCategory/'+category);
  }

  //Get Product ById
  getProductById(id:any):Observable<any[]>{
    let HTTPOptions:Object = {
      headers: new HttpHeaders({
          'Content-Type': 'application/json',
          
      })
      
   }
    return this.http.get<any>(this.APIUrl+'/Shopping/GetByProductId/'+id,HTTPOptions);
  }

  //Get Cart Items
  getCart(id:any,token:string):Observable<any[]>{
    let HTTPOptions:Object = {
      headers: new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization':`Bearer ${token}`,
      })
      
   }
    return this.http.get<any>(this.APIUrl+'/cart/GetCartItems/'+id,HTTPOptions);
  }

  //Get Edit Items
  getEditInfo(id:any,token:string):Observable<any[]>{
    let HTTPOptions:Object = {
      headers: new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization':`Bearer ${token}`,
      })
      
   }
    return this.http.get<any>(this.APIUrl+'/cart/GetEditInfo/'+id,HTTPOptions);
  }

  //Check Cart Item Duplication
  checkDuplication(body:any,token:string):Observable<any[]>{
    let HTTPOptions:Object = {
      headers: new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization':`Bearer ${token}`,
      })
      
   }
    return this.http.post<any>(this.APIUrl+'/cart/CheckItemDuplication',body,HTTPOptions);
  }


  //delete item from cart
  deleteItem(id:any,token:string):Observable<any[]>{
    let HTTPOptions:Object = {
      headers: new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization':`Bearer ${token}`,
      })
      
   }
    return this.http.delete<any>(this.APIUrl+'/cart/DeleteItem/'+id,HTTPOptions);
  }

  //add To cart
  addToCart(body:any,token:string):Observable<any[]>{
    let HTTPOptions:Object = {
      headers: new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization':`Bearer ${token}`,
      })
      
   }
    return this.http.post<any>(this.APIUrl+'/cart/AddCartItems',body,HTTPOptions);
  }

  //update item from cart
  updateQuantity(body:any,token:string):Observable<any[]>{
    let HTTPOptions:Object = {
      headers: new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization':`Bearer ${token}`,
      })
      
   }
    return this.http.post<any>(this.APIUrl+'/cart/UpdateItem',body,HTTPOptions);
  }

  //Make Purchase
  purchase(body:any,token:string):Observable<any[]>{
    let HTTPOptions:Object = {
      headers: new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization':`Bearer ${token}`,
      })
      
   }
    return this.http.post<any>(this.APIUrl+'/cart/Purchase',body,HTTPOptions);
  }


  
}
