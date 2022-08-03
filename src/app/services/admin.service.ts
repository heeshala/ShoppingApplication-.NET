import { Injectable } from '@angular/core';
import {HttpClient, HttpClientModule, HttpEvent, HttpHeaders} from '@angular/common/http';
import { Router } from '@angular/router';
import {Observable} from 'rxjs';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  readonly APIUrl="https://localhost:44323/api"
  errorMessage: any;

  constructor(private http:HttpClient,private router: Router) { }

  loginStatus:any;
 loginAdmin(body:any):Observable<any>{
  console.log("Login Called");
  console.log(body); 
  let HTTPOptions:Object = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json'
    }),
    responseType: 'text'
 }
  
  return this.http.post<any>(this.APIUrl+'/admin/AdminLogin',body,HTTPOptions);

}


getAdminInfo(body:any,token:any):Observable<any>{
  console.log("getAdminInfo");
  console.log(body); 
  let HTTPOptions:Object = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization':`Bearer ${token}`,
    }),
    responseType: 'text'
 }
  
  return this.http.post<any>(this.APIUrl+'/admin/AdminInformation',body,HTTPOptions);

}


//Get All  products
getProducts(token:string):Observable<any[]>{
  let HTTPOptions:Object = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization':`Bearer ${token}`,
    })
    
 }
  return this.http.get<any>(this.APIUrl+'/admin/AllProducts',HTTPOptions);
}


//Get All  products
getAllSellers(token:string):Observable<any[]>{
  let HTTPOptions:Object = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization':`Bearer ${token}`,
    })
    
 }
  return this.http.get<any>(this.APIUrl+'/admin/AllSellers',HTTPOptions);
}

//Add Product

addProduct(body: any,token:any): Observable<any> {
  
 
  let HTTPOptions:Object = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization':`Bearer ${token}`,
    })
    
 }
  return this.http.post<any>(this.APIUrl+'/admin/AddNewProduct',body,HTTPOptions);
}

//Upload File
upload(file: File,token:any,id:any): Observable<HttpEvent<any>> {
  const formData: FormData = new FormData();
  formData.append('file', file);
  let HTTPOptions:Object = {
    headers: new HttpHeaders({
        
        'Authorization':`Bearer ${token}`,
    })
    
 }
  return this.http.post<any>(this.APIUrl+'/admin/UpdateImage/'+id,formData,HTTPOptions);
}


//Update Stock
updateStock(body: any,token:any): Observable<any> {
  
 
  let HTTPOptions:Object = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization':`Bearer ${token}`,
    })
    
 }
  return this.http.post<any>(this.APIUrl+'/admin/UpdateStock',body,HTTPOptions);
}

}
