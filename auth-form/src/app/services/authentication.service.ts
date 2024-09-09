import { Injectable } from '@angular/core';

type SignUpModel = {
  fullName: string
  login: string;
  password: string;
}

type SignInModel = {
  login: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
  private signInEndpoint: string = "https://localhost:7259/api/authentication/sign-in";
  private signUpEndpoint: string = "https://localhost:7259/api/authentication/sign-up";
  private defaultContentType: string = "application/json";

  constructor() { }

  public async signIn(login: string, password: string) : Promise<void> {
    const model: SignInModel = {
      login: login,
      password: password
    }

    return await this.authenticationProcess(JSON.stringify(model));
  }

  public async signUp(fullName: string, login: string, password: string) : Promise<void> {
    const model : SignUpModel = {
      fullName: fullName,
      login: login,
      password: password
    }
    
    return await this.authenticationProcess(JSON.stringify(model))
  }

  public checkAuthentication() : boolean {
    return false;
  }

  private async authenticationProcess(jsonData: string) : Promise<void> {
    const token = await this.makeRequest(jsonData, this.signUpEndpoint);
    localStorage.setItem('accessToken', token);
  }

  private async makeRequest(jsonData: string, url: string) : Promise<string> {
    const response = await fetch(this.signUpEndpoint, {
      method: 'post',
      body: jsonData,
      headers: {
        'content-type': this.defaultContentType
      }
    });
    return await response.json()
  }
}