import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, Validators} from '@angular/forms';
import {HttpClient} from '@angular/common/http';
import {ActivatedRoute} from '@angular/router';
import {MyConfig} from '../../../my-config';
import {
  CountryGetAllEndpointService,
  CountryGetAllResponse
} from '../../../endpoints/country-endpoints/country-get-all-endpoint.service';

export interface StudentGetIdResponse {
  id: number;
  birthMunicipality: string;
  birthMunicipalityId: number;
  phone: string;
  dateOfBirth: string;
  countryId: number;
  firstName: string;
  lastName: string;
}

export interface MunicipalityResponse {
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
  form = new FormGroup({
    dateOfBirth: new FormControl(''),
    dobValidator: new FormControl(Date.now(), [Validators.min((new Date('1950-01-01')).getTime()), Validators.max((new Date('2019-01-01')).getTime())]),
    municipalityId: new FormControl(0),
    countryId: new FormControl(0),
    phone: new FormControl('', [Validators.pattern(/^06\d-\d\d\d-\d\d\d$/)])
  })

  student: StudentGetIdResponse | null = null;
  countries: CountryGetAllResponse[] = [];
  municipalites: MunicipalityResponse[] = [];

  constructor(private formBuilder: FormBuilder,
              private httpClient: HttpClient,
              private route: ActivatedRoute,
              private countryGetAllService : CountryGetAllEndpointService) {}

  ngOnInit(): void {
    this.form.get('countryId')!.valueChanges.subscribe(
      data => {
        this.loadMunicipalities(data!);
      })

    this.form.get('dateOfBirth')?.valueChanges.subscribe((dateOfBirth) => {
      this.form.get('dobValidator')!.setValue(new Date(dateOfBirth!).getTime());
    })

        this.route.params.subscribe(params => {
          let id = params['id'];
          if(id)
          {
            this.httpClient.get<StudentGetIdResponse>(`${MyConfig.api_address}/students/get/${id}`).subscribe({
              next: data => {
                this.student = data;

                console.log(data);

                this.form.patchValue({
                  dateOfBirth: this.student.dateOfBirth.split("T")[0],
                  countryId: this.student.countryId,
                  municipalityId: this.student.birthMunicipalityId,
                  phone: this.student.phone,
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

  loadMunicipalities(countryId: number = 1): void {
    this.httpClient.get<MunicipalityResponse[]>(`${MyConfig.api_address}/municipalities/${countryId}`).subscribe({
      next: data => {
        this.municipalites = data;
      },
      error: (error) => console.error('Error loading municipalities', error),
    })
  }


  updateStudent() {
    let student = {
      id: this.student?.id!,
      municipalityId: this.form.get('municipalityId')!.value,
      phone: this.form.get('phone')!.value,
      dateOfBirth: this.form.get('dateOfBirth')!.value,
    }

    this.httpClient.put<StudentGetIdResponse>(`${MyConfig.api_address}/students/edit`, student).subscribe({
      next: (data : StudentGetIdResponse) => {
        this.student = data;
        alert("Successfully updated student");
      },
      error: (error) => console.error('Error updating student', error),
    })
  }
}
