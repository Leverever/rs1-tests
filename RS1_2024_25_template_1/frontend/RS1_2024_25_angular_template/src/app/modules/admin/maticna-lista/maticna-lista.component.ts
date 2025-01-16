import {Component, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ActivatedRoute, Router} from '@angular/router';
import {MyConfig} from '../../../my-config';
import {StudentGetIdResponse} from '../student-edit/student-edit.component';
import {MatTableDataSource} from '@angular/material/table';
import {FormBuilder, FormControl, FormGroup, Validators} from '@angular/forms';
import {MyAuthService} from '../../../services/auth-services/my-auth.service';

export interface YOSGetResponse {
  id: number
  akademskaGodina: string;
  godinaStudija: number;
  obnova: boolean;
  datumUpisa: string;
  snimio: string;
  datumOvjere?: string;
  komentar?: string;
}

export interface AcademicYearGetResponse {
  id: number,
  name: string,
  startDate: string,
  endDate: string
}

@Component({
  selector: 'app-maticna-lista',
  standalone: false,

  templateUrl: './maticna-lista.component.html',
  styleUrl: './maticna-lista.component.css'
})
export class MaticnaListaComponent implements OnInit {
  student: StudentGetIdResponse | null = null;
  yosList: YOSGetResponse[] = []
  academicYears: AcademicYearGetResponse[] = [];

  dataSource = new MatTableDataSource<YOSGetResponse>();
  displayedColumns = ["id", "akademska", "godinaStudija", "obnova", "datumUpisa", "snimio", "actions"];
  creationHidden = true;

  form = new FormGroup({
    studentId: new FormControl(0, [Validators.required]),
    datumUpisa: new FormControl('', [Validators.required]),
    godinaStudija: new FormControl<number>(0,[Validators.required, Validators.min(1), Validators.max(5)]),
    akademskaGodinaId: new FormControl(0, [Validators.required]),
    obnova: new FormControl(false),
    cijenaSkolarine: new FormControl(0),
  })

  valid = true;

  constructor(private httpClient: HttpClient, private route: ActivatedRoute,
              private router: Router,
              private builder: FormBuilder,
              private auth : MyAuthService) {
  }

  ngOnInit(): void {
    this.form.get('obnova')!.disable();
    this.form.get('cijenaSkolarine')!.disable();
    this.form.get('datumUpisa')!.setValue(new Date(Date.now()).toISOString().split('T')[0]);

    this.httpClient.get<AcademicYearGetResponse[]>(`${MyConfig.api_address}/academic-years/get-all`).subscribe({
      next: data => {
        this.academicYears = data;
        this.form.get('akademskaGodinaId')!.setValue(this.academicYears[0].id);
      },
      error: err => {
        console.log(err);
      }
    })

    this.form.get('datumUpisa')!.valueChanges.subscribe(data => {
      /*
      let akademska = this.academicYears.find(val => val.id == this.form.get('akademskaGodinaId')!.value);
      if(akademska && data)
      {
        let millis = new Date(data!).getTime();
        let minMillis = new Date(akademska.startDate).getTime();
        let maxMillis = new Date(akademska.endDate).getTime();

        this.valid = !(millis < maxMillis || millis > minMillis);
        console.log(millis > maxMillis || millis < minMillis);
      }
       */
    });

    this.form.get('godinaStudija')!.valueChanges.subscribe({
      next: godina => {
        if((godina! < 1 || godina! > 5) && godina! != null) {
          this.form.get('godinaStudija')!.setValue(1);
        }

        let obnova = this.yosList.find(yos => yos.godinaStudija == godina) != undefined;


        this.form.get('obnova')!.setValue(obnova);
        this.form.get('cijenaSkolarine')!.setValue(obnova ? 400 : 1800);
      }
    })

    this.route.params.subscribe(params => {
      let id = params['id'];
      if(id)
      {
        this.httpClient.get<StudentGetIdResponse>(`${MyConfig.api_address}/students/get/${id}`).subscribe({
          next: data => {
            this.student = data;

            this.form.get('studentId')!.setValue(this.student!.id);

            console.log(data);

            this.httpClient.get<YOSGetResponse[]>(`${MyConfig.api_address}/yos/get/${id}`).subscribe({
              next: data => {
                this.yosList = data;
                this.dataSource.data = this.yosList;
                this.form.get('godinaStudija')!.setValue(1);
                console.log(this.yosList);
              }
            })
          }
        })
      }
    })
    }


  ovjeri(id: number) {
    this.router.navigate(["/admin/verify-semester", id])
  }

  getDate(datumUpisa: string) {
    return new Date(datumUpisa).toLocaleDateString();
  }

  showHideCreate() {
    this.creationHidden = !this.creationHidden;
  }

  createYos() {
    if(this.form.invalid)
    {
      return;
    }

    let yos = {
      studentId: this.student!.id,
      datumUpisa: this.form.get('datumUpisa')!.value,
      godinaStudija: this.form.get('godinaStudija')!.value,
      akademskaGodinaId: this.form.get('akademskaGodinaId')!.value,
      snimioId: this.auth.getMyAuthInfo()!.userId
    }

    this.httpClient.post<YOSGetResponse>(`${MyConfig.api_address}/yos/create`, yos).subscribe({
      next: val => {
        this.yosList.push(val);
        this.dataSource.data = this.yosList;
        this.form.get('godinaStudija')!.setValue(1);
      },
      error: (error) => { console.error('Error creating yos'); }
    })
  }
}
