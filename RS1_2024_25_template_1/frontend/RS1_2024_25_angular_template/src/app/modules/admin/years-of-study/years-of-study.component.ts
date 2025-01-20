import {Component, OnInit} from '@angular/core';
import {
  StudentGetAllEndpointService,
  StudentGetAllResponse
} from '../../../endpoints/student-endpoints/student-get-all-endpoint.service';
import {ActivatedRoute, Router} from '@angular/router';
import {MatDialog} from '@angular/material/dialog';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';
import {MatSnackBar} from '@angular/material/snack-bar';
import {debounceTime} from 'rxjs';
import {MyConfig} from '../../../my-config';
import {MatTableDataSource} from '@angular/material/table';
import {FormControl, FormGroup, Validators} from '@angular/forms';

export interface YOSGetResponse {
  id: number;
  godinaStudija: number;
  datumUpisa: string;
  obnova: boolean;
  akademskaGodina: string;
  snimio: string;
  studentId: number;
  datumOvjere?:string;
  napomena?: string;
}

export interface AcademicYearGetResponse {
  id: number;
  name: string;
  startDate: string;
  endDate: string;
}

@Component({
  selector: 'app-years-of-study',
  standalone: false,

  templateUrl: './years-of-study.component.html',
  styleUrl: './years-of-study.component.css'
})
export class YearsOfStudyComponent implements OnInit{
  student : StudentGetAllResponse | null = null;
  yos: YOSGetResponse[] = [];
  academicYears : AcademicYearGetResponse[] = [];

  dataSource = new MatTableDataSource<YOSGetResponse>()
  displayedColumns = ["id","akademska","godina","obnova", "datumUpisa","snimio", "actions"];

  showForm = false;

  selectedAcademicYear : AcademicYearGetResponse | null = null;

  form = new FormGroup({
    datumUpisa: new FormControl(new Date(Date.now()).toISOString().split('T')[0], Validators.required),
    duValidator: new FormControl(0),
    godinaStudija: new FormControl(0, [Validators.required, Validators.min(1), Validators.max(5)]),
    akademskaGodinaId: new FormControl(1, Validators.required),
    cijenaSkolarine: new FormControl(0),
    obnova: new FormControl(false),
  });


  constructor(private router: Router,
              private studentGetAll: StudentGetAllEndpointService,
              private dialog: MatDialog,
              private http: HttpClient,
              private snackBar: MatSnackBar,
              private route: ActivatedRoute,) {
  }

  ngOnInit(): void {
    this.loadStudent();
    this.form.get('godinaStudija')?.valueChanges.subscribe(godina => {
      if(godina){
        let renewal = this.yos.find(val => val.godinaStudija == godina) !== undefined;
        this.form.get('obnova')!.setValue(renewal);
        this.form.get('cijenaSkolarine')!.setValue(renewal ? 400 : 1800);
      }
    })

    this.form.get('akademskaGodinaId')?.valueChanges.subscribe(value => {
      if(value)
      {
        this.form.get('datumUpisa')!.setValue(new Date(Date.now()).toISOString().split('T')[0]);
        this.selectedAcademicYear = this.academicYears.find(val => val.id == value) ?? null;
        if(this.selectedAcademicYear)
        {
          this.form.get('duValidator')!.setValidators([Validators.min(new Date(this.selectedAcademicYear.startDate).getTime()),
            Validators.max(new Date(this.selectedAcademicYear.endDate).getTime())]);
        }
      }
    })

    this.form.get('datumUpisa')!.valueChanges.subscribe(value => {
      if(value)
      {
        this.form.get('duValidator')!.setValue(new Date(value).getTime());
      }
    })

    this.form.get('obnova')!.disable();
    this.form.get('cijenaSkolarine')!.disable();
  }

  loadStudent(){
    this.route.params.subscribe(params => {
      let id = params['id'];
      if(id){
        this.http.get<StudentGetAllResponse>(`${MyConfig.api_address}/students/get/${id}`).subscribe({
          next: (data) => {
            this.student = data;

            this.http.get<YOSGetResponse[]>(`${MyConfig.api_address}/yos/get/${id}`).subscribe({
              next: (val) => {
                this.yos = val;
                this.dataSource.data = this.yos;
                this.form.get('godinaStudija')?.setValue(1);

                this.http.get<AcademicYearGetResponse[]>(`${MyConfig.api_address}/yos/ay/`).subscribe({
                  next: value => {
                    this.academicYears = value;
                    this.form.get('akademskaGodinaId')?.setValue(value[0].id);
                  },
                  error: err => console.log(err.message)
                })
              }
            })
          },
          error: (error:HttpErrorResponse) => {
            console.log(error.message);
          }
        })
      }
    })


  }

  ovjeriSemestar(id:number) {
    this.router.navigate(["/admin/ovjeri", id])
  }

  pokaziFormu() {
    this.showForm = !this.showForm;
  }

  upisiGodinu() {
    if(this.form.invalid)
    {
      console.log("Forma je invalidna");
      return;
    }

    let req = {
      studentId: this.student!.id,
      godinaStudija: this.form.get('godinaStudija')!.value,
      datumUpisa: this.form.get('datumUpisa')!.value,
      akademskaGodinaId: this.form.get('akademskaGodinaId')!.value,
    }

    this.http.post<YOSGetResponse>(`${MyConfig.api_address}/yos/create`, req).subscribe({
      next: (data) => {
        this.yos.push(data);
        this.dataSource.data = this.yos;
        this.form.get('godinaStudija')?.setValue(1);
        this.snackBar.open(`Student ${this.student?.studentNumber} upisan na ${data.godinaStudija}. godinu studija`, "", {duration: 2000});
      },
      error: (error:HttpErrorResponse) => {
        this.snackBar.open(error.message, "", {duration: 2000});
      }
    })
  }
}
