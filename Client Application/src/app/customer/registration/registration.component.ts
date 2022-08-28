import { Component, OnInit } from '@angular/core';
import { SharedService } from 'src/app/shared.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
//import validator and FormBuilder
import { FormGroup, Validators, FormBuilder } from '@angular/forms';


@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css']
})
export class RegistrationComponent implements OnInit {

//Create FormGroup
requiredForm: FormGroup= this.fb.group({
  name: ['', Validators.compose([Validators.required, Validators.minLength(3), Validators.maxLength(50)])],
  email: ['', Validators.compose([Validators.required, Validators.email])],
  address: ['', Validators.required],
  dob: ['', Validators.required],
  password: ['', Validators.required],
  contact:['', Validators.compose([Validators.required, Validators.minLength(9), Validators.maxLength(9)])]
});

  name:any;
  email:any;
  address:any;
  contact:any;
  dob:any;
  password:any;


  constructor(private service:AuthenticationService,private fb: FormBuilder) {
   
   }


  

  ngOnInit(): void {
    
  }

  register(){
    

    console.log(this.name);
    console.log(this.email);
    console.log(this.address);
    console.log(this.contact);
    console.log(this.dob);
    console.log(this.password);

    const body={ Name: this.name,Email:this.email,Address:this.address,Contact:"+94"+this.contact,DOB:this.dob,Password:this.password }
    this.service.addNewUser(body);
  }

}
