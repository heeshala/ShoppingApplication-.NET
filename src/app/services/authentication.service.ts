import { Injectable } from '@angular/core';
import {HttpClient, HttpClientModule, HttpHeaders} from '@angular/common/http';
import { Router } from '@angular/router';
import {Observable} from 'rxjs';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {
  readonly APIUrl="https://localhost:44323/api"
  errorMessage: any;

  constructor(private http:HttpClient,private router: Router) { }
  postId:any;

  //Register Account
  addNewUser(body:any){
    console.log("Called");
    return this.http.post<any>(this.APIUrl+'/customer/CustomerRegistration', body).subscribe({
      next: data => {
        
          this.postId = data;
          console.error(this.postId);
          if(this.postId==1){
            Swal.fire({
              position: 'center',
              icon: 'success',
              title: 'Registration Successfull',
              showConfirmButton: false,
              timer: 1500
            })
            this.router.navigate(["login"]);
          }
      },
      error: error => {
          this.errorMessage = error.message;
          console.error('There was an error!', error);
      }
  })  
}


//Login 

loginStatus:any;
 loginCustomer(body:any):Observable<any>{
  console.log("Login Called");
  console.log(body); 
  let HTTPOptions:Object = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json'
    }),
    responseType: 'text'
 }
  
  return this.http.post<any>(this.APIUrl+'/customer/CustomerLogin',body,HTTPOptions);

}

//Get user Info
getUserInfo(body:any,token:any):Observable<any>{
  console.log("Login Called");
  console.log(body); 
  let HTTPOptions:Object = {
    headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization':`Bearer ${token}`,
    }),
    responseType: 'text'
 }
  
  return this.http.post<any>(this.APIUrl+'/customer/UserInformation',body,HTTPOptions);

}
}
