import { Routes } from '@angular/router';
import { IndexComponent } from './pages/home/index/index.component';
import { SignInComponent } from './pages/authentication/sign-in/sign-in.component';
import { SignUpComponent } from './pages/authentication/sign-up/sign-up.component';
import { UserListComponent } from './pages/user/user-list/user-list.component';
import { UserInformationComponent } from './pages/user/user-information/user-information.component';
import { AboutComponent } from './pages/home/about/about.component';
import { NotFoundError } from 'rxjs';
import { authenticationServiceGuard } from './services/authentication/authentication-service.guard';

export const routes: Routes = [
    { path: '', component: IndexComponent },
    {
        path: 'authentication', children: [
            { path: 'sign-in', component: SignInComponent },
            { path: 'sign-up', component: SignUpComponent }
        ] 
    },
    { 
        path: 'user', resolve: [ authenticationServiceGuard ] , children: [
            { path: 'user-list', component: UserListComponent },
            { path: 'user-information', component: UserInformationComponent }
    ] },
    { 
        path: 'home', resolve: [ authenticationServiceGuard ], children: [
            { path: 'about', component: AboutComponent }
        ]
    },
    { path: '**', component: NotFoundError }
];