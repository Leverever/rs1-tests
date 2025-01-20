import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormControl} from '@angular/forms';
import {ActivatedRoute, Router} from '@angular/router';
import {StudentGetAllEndpointService} from '../../../endpoints/student-endpoints/student-get-all-endpoint.service';
import {MatDialog} from '@angular/material/dialog';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {MatSnackBar} from '@angular/material/snack-bar';
import {MyPageProgressbarComponent} from '../../shared/progressbars/my-page-progressbar/my-page-progressbar.component';
import {MyConfig} from '../../../my-config';
import {Location} from '@angular/common';

@Component({
  selector: 'app-verify-semester',
  standalone: false,

  templateUrl: './verify-semester.component.html',
  styleUrl: './verify-semester.component.css'
})
export class VerifySemesterComponent implements OnInit {
  napomena = new FormControl<string | null>(null);
  id: number | null = null;

  constructor(private router: Router,
              private studentGetAll: StudentGetAllEndpointService,
              private dialog: MatDialog,
              private http: HttpClient,
              private snackBar: MatSnackBar,
              private route: ActivatedRoute,
              protected location: Location) {
  }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      let id = params['id'];
      if(id){
        this.id = id;
      }
    });
  }


  ovjeri() {
    if(this.id != null)
    {
      let req = {
        id : this.id,
        napomena : this.napomena.value ?? undefined,
      }

      this.http.put<any>(`${MyConfig.api_address}/yos/verify`, req).subscribe({
        next: (data) => {
          this.snackBar.open("Semestar ovjeren", "", {duration: 2000});
          this.location.back();
        },
        error: (error:HttpErrorResponse) => {
          this.snackBar.open(error.message, "", {duration: 2000});
        }
      })
    }
  }
}
