import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';

import { CookieService } from 'ngx-cookie-service';
import { Router } from '@angular/router';

import Swal from 'sweetalert2';
import { AdminService } from '../services/admin.service';
@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit {

  constructor(private service:AdminService,private fb: FormBuilder,private cookieService: CookieService,private router: Router) { }
role:any;
  ngOnInit(): void {
    this.role=this.cookieService.get('role');
    if(this.role=='admin'){
      this.router.navigate(["dashboard"]);
    }
  }
//Create FormGroup
requiredForm: FormGroup= this.fb.group({
  uname: ['', Validators.compose([Validators.required])], 
  password: ['', Validators.required]
  
});

uname:any;
password:any;

resultStatus:any;
  async login(){
    console.log(this.uname);
    console.log(this.password);

    const body={ Username:this.uname,Password:this.password }
   this.service.loginAdmin(body).subscribe(data=>{
    this.resultStatus=data;
    console.log(this.resultStatus);
    if(this.resultStatus!="User Credentials Do Not Match" && this.resultStatus!="User Do Not Exist" ){

      //create cookie
      this.cookieService.set('token', this.resultStatus,1);

      //User Info
      this.service.getAdminInfo(body,this.resultStatus).subscribe(data=>{
       console.log(data);
       let myObj = JSON.parse(data);
       this.cookieService.delete('customerId');
       this.cookieService.delete('customerName');
       this.cookieService.delete('role');
       

       this.cookieService.set('adminId', myObj[0].AdminId,1);
       this.cookieService.set('adminName', myObj[0].Name,1);
       this.cookieService.set('role', 'admin',1);
       this.router.navigate(["dashboard"]).then(() => {
        window.location.reload();
      });
       
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
