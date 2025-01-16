import {Component, OnInit} from '@angular/core';
import {FormControl, FormGroup} from '@angular/forms';
import {HttpClient} from '@angular/common/http';
import {ActivatedRoute} from '@angular/router';
import {Location} from '@angular/common';
import {YOSGetResponse} from '../maticna-lista/maticna-lista.component';
import {MyConfig} from '../../../my-config';

@Component({
  selector: 'app-verify-semester',
  standalone: false,

  templateUrl: './verify-semester.component.html',
  styleUrls: ['../maticna-lista/maticna-lista.component.css','./verify-semester.component.css']
})
export class VerifySemesterComponent implements OnInit {
  napomena = new FormControl('')
  semesterId : number | null = null;

  constructor(private http: HttpClient,
              private route: ActivatedRoute,
              private location: Location) { }

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      let id = params['id'];
      if(id)
      {
        this.semesterId = id;
      }
    })
  }

  ovjeri() {
    this.http.post<YOSGetResponse>(`${MyConfig.api_address}/yos/verify`, {semesterId: this.semesterId, comment: this.napomena.value}).subscribe({
      next: data => {
        alert("Success");
        console.log(data);
        this.location.back();
      }
    })
  }

  return() {
    this.location.back();
  }
}
