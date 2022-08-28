import { Injectable } from '@angular/core';
import {HttpClient, HttpClientModule, HttpHeaders} from '@angular/common/http';
import { Router } from '@angular/router';
import {Observable} from 'rxjs';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
   readonly APIUrl="https://localhost:44323/api"
  errorMessage: any;
  constructor(private http:HttpClient,private router: Router) { }
  postId:any;



  //Get All the Items List
  getAllProducts():Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/Shopping');
  }

  //Get History Items List
  getHistory(id:any,token:string):Observable<any[]>{
    let HTTPOptions:Object = {
      headers: new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization':`Bearer ${token}`,
      })
      
   }
    return this.http.get<any>(this.APIUrl+'/customer/CustomerOrders/'+id,HTTPOptions);
  }

  //Get Invoice Items Details
  getInvoice(id:any,token:string):Observable<any[]>{
    let HTTPOptions:Object = {
      headers: new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization':`Bearer ${token}`,
      })
      
   }
    return this.http.get<any>(this.APIUrl+'/customer/OrderInvoice/'+id,HTTPOptions);
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

  
}
