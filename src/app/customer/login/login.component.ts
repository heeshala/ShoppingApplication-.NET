import { Component, OnInit } from '@angular/core';
//import validator and FormBuilder
import { FormGroup, Validators, FormBuilder } from '@angular/forms';

import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';

import Swal from 'sweetalert2';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  providers: [CookieService]
})
export class LoginComponent implements OnInit {
//Create FormGroup
requiredForm: FormGroup= this.fb.group({
  email: ['', Validators.compose([Validators.required, Validators.email])], 
  password: ['', Validators.required]
  
});



  productId:any;
  email:any;
  password:any;
  constructor(private service:AuthenticationService,private fb: FormBuilder,private cookieService: CookieService,private router: Router) { }

  ngOnInit(): void {
    this.productId=history.state.data;
  }
 

  
  resultStatus:any;
  async login(){
    console.log(this.email);
    console.log(this.password);

    const body={ Email:this.email,Password:this.password }
   this.service.loginCustomer(body).subscribe(data=>{
    this.resultStatus=data;
    console.log(this.resultStatus);
    if(this.resultStatus!="User Credentials Do Not Match" && this.resultStatus!="User Do Not Exist" ){
      this.cookieService.delete('token');
      //create cookie
      this.cookieService.set('token', this.resultStatus,1);

      //User Info
      this.service.getUserInfo(body,this.resultStatus).subscribe(data=>{
       console.log(data);
       let myObj = JSON.parse(data);
       this.cookieService.delete('adminId');
       this.cookieService.delete('adminName');
       this.cookieService.delete('role');
       
       this.cookieService.set('customerId', myObj[0].CustomerId,1);
       this.cookieService.set('customerName', myObj[0].Name,1);
       this.cookieService.set('role', 'customer',1);
       if(this.productId>0){
        
        this.router.navigate(['item'], {
          state:{
            data: this.productId
          }
        }).then(() => {
          window.location.reload();
        });
       }else{
        this.router.navigate(["orders"]).then(() => {
          window.location.reload();
        });
        
       }
       
      });

      
      
    }else{
      
      console.log("No")
      Swal.fire({
        position: 'center',
        icon: 'error',
        title: data,
        showConfirmButton: false,
        timer: 1500
      });
    }
  });

   };
    


}
