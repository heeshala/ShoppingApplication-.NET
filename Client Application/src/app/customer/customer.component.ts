import { Component, OnInit } from '@angular/core';

import { FormGroup, Validators, FormBuilder } from '@angular/forms';

import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';

import Swal from 'sweetalert2';
import { CustomerService } from '../services/customer.service';
@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.css']
})
export class CustomerComponent implements OnInit {

  constructor(private service:CustomerService,private fb: FormBuilder,private cookieService: CookieService,private router: Router) { }

  requiredForm: FormGroup= this.fb.group({
    
    confirm: ['', Validators.required],
    password: ['', Validators.required]
  });
  
    password:any;
    confirm:any;
    

  customerId:any;
  token:any;
  ngOnInit(): void {
    if(this.cookieService.check('token')){
      if(this.cookieService.get('role')=='customer'){
        this.customerId=this.cookieService.get('customerId');
        this.token=this.cookieService.get('token');
        console.log(this.token);
        this.loadpersonaInfo();
      }else{
        console.log("nocookie")
      this.router.navigate(["login"]);
      }
      
      
    }else{
      console.log("nocookie")
      this.router.navigate(["login"]);
    }
  }

  logout(){
    this.cookieService.delete('customerId');
       this.cookieService.delete('customerName');
       this.cookieService.delete('role');
       this.cookieService.delete('token');

       this.router.navigate(["products"]).then(() => {
        window.location.reload();
      });
  }

  personalInfo:any=[];

  loadpersonaInfo(){
    this.service.getpersonalProfile(this.customerId,this.token).subscribe(data=>{
      this.personalInfo=data;
      
    });
  }

  clearfields(){
    this.password="";
    this.confirm="";
  }
  status:any;
  updatePassword(){
    const body={CustomerId:this.customerId,Password:this.password}
    if(this.password==this.confirm){
      this.service.changePassword(body,this.token).subscribe(data=>{
        this.status=data;

        if(this.status==1){
          Swal.fire({
            position: 'center',
            icon: 'success',
            title: "Password Changed Successfully",
            showConfirmButton: false,
            timer: 1500
          }).then(() => {
            window.location.reload();
          });
        }else{
          Swal.fire({
            position: 'center',
            icon: 'error',
            title: "Unable to Change Password",
            showConfirmButton: false,
            timer: 1500
          }).then(() => {
            window.location.reload();
          });
        }
        
      });
    }else{
      Swal.fire({
        position: 'center',
        icon: 'error',
        title: "Password and Confirmation do not match",
        showConfirmButton: false,
        timer: 1500
      }).then(() => {
        window.location.reload();
      });
    }
  }

}
