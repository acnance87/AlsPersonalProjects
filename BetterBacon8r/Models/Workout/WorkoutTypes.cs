﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AlsProjects.Models.Workouts;

public partial class WorkoutTypes
{
    public int WorkoutTypeID { get; set; }

    public string ExerciseName { get; set; } = string.Empty;

    public virtual ICollection<Workouts> Workouts { get; set; } = [];
}