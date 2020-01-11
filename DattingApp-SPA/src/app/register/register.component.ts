import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_service/auth.service';
import { AlertifyService } from '../_service/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any; // To pass down parameters from the parent.
  @Output() cancelRegister = new EventEmitter(); // Emit an event for the parent to capture and act upon it.
  model: any = {};
  registerForm: FormGroup;


  constructor(private authService: AuthService,
              private alertify: AlertifyService,
              private fb: FormBuilder) { }
  ngOnInit() {
    /*
    this.registerForm = new FormGroup({
      username: new FormControl('', Validators.required),
      password: new FormControl('',
        [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
      confirmPassword: new FormControl('', Validators.required)
    }, this.passwordMatchValidator);
    */
   this.createRegisterForm();
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : {mismatch: true};
  }

  createRegisterForm() {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    }, { validator: this.passwordMatchValidator});
  }
// , Validators.minLength(4), Validators.maxLength(8)]],
  register() {
    /*
    this.authService.register(this.model).subscribe(() => {
      // console.log('registration ok');
      this.alertify.success('Registration Succesfull');
    }, error => {
      // console.log('error registering!', error);
      this.alertify.error('Error registering');
    });
    */
   console.log(this.registerForm.value);
  }
  cancel() {
    this.cancelRegister.emit(false);
    // console.log('cancelled');
  }
}
