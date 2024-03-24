class User {
  final String firstName;
  final String lastName;
  final String jwt;

  const User(
      {required this.firstName, required this.lastName, required this.jwt});

  factory User.fromJson(Map<String, dynamic> json) => User(
      firstName: json['firstName'],
      lastName: json['lastName'],
      jwt: json['token']);

  Map<String, dynamic> toJson() =>
      {"firstName": firstName, "lastName": lastName, "token": jwt};
}
