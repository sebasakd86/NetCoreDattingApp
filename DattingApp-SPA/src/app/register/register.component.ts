import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_service/auth.service';
import { AlertifyService } from '../_service/alertify.service';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder
} from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() valuesFromHome: any; // To pass down parameters from the parent.
  @Output() cancelRegister = new EventEmitter(); // Emit an event for the parent to capture and act upon it.
  user: User;
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>; // To make all of the ppties optional and have kinda of an empty obj

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router
  ) {}
  ngOnInit() {
    /*
    this.registerForm = new FormGroup({
      username: new FormControl('', Validators.required),
      password: new FormControl('',
        [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
      confirmPassword: new FormControl('', Validators.required)
    }, this.passwordMatchValidator);
    */
    this.bsConfig = {
      containerClass: 'theme-red'
    };
    this.createRegisterForm();
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value
      ? null
      : { mismatch: true };
  }

  createRegisterForm() {
    this.registerForm = this.fb.group(
      {
        gender: ['male'],
        username: ['', Validators.required],
        knownAs: ['', Validators.required],
        dateOfBirth: [null, Validators.required],
        city: ['', Validators.required],
        country: ['', Validators.required],
        password: ['', Validators.required],
        confirmPassword: ['', Validators.required]
      },
      { validator: this.passwordMatchValidator }
    );
  }
  // , Validators.minLength(4), Validators.maxLength(8)]],
  register() {
    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);
      this.authService.register(this.user).subscribe(
        () => {
          this.alertify.success('Registration successfull');
        },
        error => {
          this.alertify.error(error);
        },
        () => {
          this.authService.login(this.user).subscribe(() => {
            this.router.navigate(['/members']);
          });
        }
      );
    }
    /*
    this.authService.register(this.model).subscribe(() => {
      // console.log('registration ok');
      this.alertify.success('Registration Succesfull');
    }, error => {
      // console.log('error registering!', error);
      this.alertify.error('Error registering');
    });
    */
    // console.log(this.registerForm.value);
  }
  cancel() {
    this.cancelRegister.emit(false);
    // console.log('cancelled');
  }
}
