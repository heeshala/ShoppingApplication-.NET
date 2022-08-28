import { Injectable } from '@angular/core';
import {HttpClient, HttpClientModule, HttpHeaders} from '@angular/common/http';
import { Router } from '@angular/router';
import {Observable} from 'rxjs';
@Injectable({
  providedIn: 'root'
})
export class CustomerService {

 
  readonly APIUrl="https://localhost:44323/api"
  errorMessage: any;

  constructor(private http:HttpClient,private router: Router) { }
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

  //Personal Profile
  getpersonalProfile(id:any,token:string):Observable<any[]>{
    let HTTPOptions:Object = {
      headers: new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization':`Bearer ${token}`,
      })
      
   }
    return this.http.get<any>(this.APIUrl+'/customer/CustomerInformation/'+id,HTTPOptions);
  }

  //Change Password
  changePassword(body:any,token:string):Observable<any[]>{
    let HTTPOptions:Object = {
      headers: new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization':`Bearer ${token}`,
      })
      
   }
    return this.http.post<any>(this.APIUrl+'/customer/ChangePassword',body,HTTPOptions);
  }
}
