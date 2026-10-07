import '../models/user.dart';
import 'api_provider.dart';

/// The signed-in user's own account (Users/Me); a user never reads other accounts.
class UserProvider extends ApiProvider {
  UserProvider() : super('Users');

  Future<User> getMe() async => User.fromJson(await getJson('$endpoint/Me'));

  Future<User> updateMe(Map<String, dynamic> request) async =>
      User.fromJson((await sendAction('PUT', '$endpoint/Me', request))!);

  Future<void> changePassword({
    required String password,
    required String newPassword,
    required String confirmNewPassword,
  }) =>
      sendAction('PUT', '$endpoint/Me/ChangePassword', {
        'password': password,
        'newPassword': newPassword,
        'confirmNewPassword': confirmNewPassword,
      });
}
