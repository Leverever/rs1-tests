import {Component, OnInit} from '@angular/core';
import {StudentGetAllResponse} from "../../../endpoints/student-endpoints/student-get-all-endpoint.service";
import {HttpClient, HttpErrorResponse} from "@angular/common/http";
import {ActivatedRoute, Router} from "@angular/router";
import {MyConfig} from "../../../my-config";
import {FormControl, FormGroup, Validators} from "@angular/forms";
import {
  CountryGetAllEndpointService,
  CountryGetAllResponse
} from "../../../endpoints/country-endpoints/country-get-all-endpoint.service";
import {CityGetByIdEndpointService} from "../../../endpoints/city-endpoints/city-get-by-id-endpoint.service";
import {
  CityUpdateOrInsertEndpointService
} from "../../../endpoints/city-endpoints/city-update-or-insert-endpoint.service";
import {RegionGetAllEndpointService} from "../../../endpoints/region-endpoints/region-get-all-endpoint.service";
import {MatSnackBar} from "@angular/material/snack-bar";

export interface MunicipalityGetResponse {
  id: number;
  name: string;
}

@Component({
  selector: 'app-student-edit',
  standalone: false,

  templateUrl: './student-edit.component.html',
  styleUrl: './student-edit.component.css'
})
export class StudentEditComponent implements OnInit{
  student: StudentGetAllResponse | null = null;
  countries: CountryGetAllResponse[] = [];
  municipalities: MunicipalityGetResponse[] = [];

  form = new FormGroup({
    dob: new FormControl('', Validators.required),
    dobValidator: new FormControl(0, [Validators.min((new Date('1900-01-01')).getTime()),Validators.max((new Date('2020-01-01')).getTime())]),
    countryId: new FormControl(0, [Validators.required]),
    municipalityId: new FormControl(0, [Validators.required]),
    phone: new FormControl('',[Validators.required, Validators.pattern(/^06\d-\d\d\d-\d\d\d$/)]),
  })


  constructor(private http: HttpClient,
              private router: Router,
              private route: ActivatedRoute,
              private cityGetByIdService: CityGetByIdEndpointService,
              private cityUpdateService: CityUpdateOrInsertEndpointService,
              private countryGetAllService: CountryGetAllEndpointService,
              private regionGetAllService: RegionGetAllEndpointService,
              private snackBar: MatSnackBar) {
  }

  ngOnInit(): void {
    this.form.get('dob')!.valueChanges.subscribe(value => {
      if(value)
      {
        this.form.get('dobValidator')!.setValue(new Date(value).getTime());
      }
    })

    this.form.get('countryId')?.valueChanges.subscribe(countryId=> {
      if(countryId)
      {
        this.loadMunicipalities(countryId);
      }
    })


    this.route.params.subscribe(params => {
      let id= params['id'];
      if(id)
      {
        this.http.get<StudentGetAllResponse>(`${MyConfig.api_address}/students/get/${id}`).subscribe({
          next: (data) => {
            this.student = data;

            this.form.patchValue({
              dob: this.student.dateOfBirth,
              phone: this.student.phone,
              countryId: this.student.countryId,
              municipalityId: this.student.birthMunicipalityId
            })
          }
        })
      }
    })

    this.loadCountries();
  }

  loadCountries(): void {
    this.countryGetAllService.handleAsync().subscribe({
      next: (countries) => (this.countries = countries),
      error: (error) => console.error('Error loading countries', error),
    });
  }

  loadMunicipalities(id: number)
  {
    this.http.get<MunicipalityGetResponse[]>(`${MyConfig.api_address}/municipalities/get/${id}`).subscribe({
      next: (data) => {
        this.municipalities = data;
        if(id != this.student?.countryId)
        {
          this.form.get('municipalityId')!.setValue(null);
        }
        else {
          this.form.get('municipalityId')!.setValue(this.student.birthMunicipalityId);
        }
      }
    })
  }

  updateStudent() {
    if (this.form.invalid)
    {
      console.log("Form is invalid");
    }

    let edit = {
      id: this.student!.id,
      municipalityId: this.form.get('municipalityId')!.value,
      phone: this.form.get('phone')!.value,
      dateOfBirth: this.form.get('dob')!.value,
    }

    this.http.put<StudentGetAllResponse>(`${MyConfig.api_address}/students/edit`, edit).subscribe({
      next: (data) => {
        this.student = data;
        this.snackBar.open(`Updated ${this.student.studentNumber} successfully.`, "", {duration: 2000});
      },
      error: (error:HttpErrorResponse) => {
        this.snackBar.open(error.message, "", {duration: 2000});
      }
    })
  }
}
